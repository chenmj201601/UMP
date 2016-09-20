//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a396c8eb-c648-4685-add5-59ab2d200409
//        CLR Version:              4.0.30319.18444
//        Name:                     AdpcmWaveFormat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                AdpcmWaveFormat
//
//        created by Charley at 2014/12/8 15:03:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Microsoft ADPCM
    /// See http://icculus.org/SDL_sound/downloads/external_documentation/wavecomp.htm
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class AdpcmWaveFormat : WaveFormat
    {
        short samplesPerBlock;
        short numCoeff;
        // 7 pairs of coefficients
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        short[] coefficients;

        /// <summary>
        /// Empty constructor needed for marshalling from a pointer
        /// </summary>
        AdpcmWaveFormat()
            : this(8000, 1)
        {
        }

        /// <summary>
        /// Samples per block
        /// </summary>
        public int SamplesPerBlock
        {
            get { return samplesPerBlock; }
        }

        /// <summary>
        /// Number of coefficients
        /// </summary>
        public int NumCoefficients
        {
            get { return numCoeff; }
        }

        /// <summary>
        /// Coefficients
        /// </summary>
        public short[] Coefficients
        {
            get { return coefficients; }
        }

        /// <summary>
        /// Microsoft ADPCM  
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Channels</param>
        public AdpcmWaveFormat(int sampleRate, int channels) :
            base(sampleRate, 0, channels)
        {
            waveFormatTag = WaveFormatEncoding.Adpcm;

            // TODO: validate sampleRate, bitsPerSample
            extraSize = 32;
            switch (this.sampleRate)
            {
                case 8000:
                case 11025:
                    blockAlign = 256;
                    break;
                case 22050:
                    blockAlign = 512;
                    break;
                default:
                    blockAlign = 1024;
                    break;
            }

            bitsPerSample = 4;
            samplesPerBlock = (short)((((blockAlign - (7 * channels)) * 8) / (bitsPerSample * channels)) + 2);
            averageBytesPerSecond =
                ((SampleRate * blockAlign) / samplesPerBlock);

            // samplesPerBlock = blockAlign - (7 * channels)) * (2 / channels) + 2;


            numCoeff = 7;
            coefficients = new short[] {
                256,0,512,-256,0,0,192,64,240,0,460,-208,392,-232
            };
        }

        /// <summary>
        /// Serializes this wave format
        /// </summary>
        /// <param name="writer">Binary writer</param>
        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
            writer.Write(numCoeff);
            foreach (short coefficient in coefficients)
            {
                writer.Write(coefficient);
            }
        }

        /// <summary>
        /// String Description of this WaveFormat
        /// </summary>
        public override string ToString()
        {
            return String.Format("Microsoft ADPCM {0} Hz {1} channels {2} bits per sample {3} samples per block",
                SampleRate, channels, bitsPerSample, samplesPerBlock);
        }
    }
}
