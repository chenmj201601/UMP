//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffdcc2fe-29f4-4c46-a40b-4f0486ba2539
//        CLR Version:              4.0.30319.18444
//        Name:                     Gsm610WaveFormat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                Gsm610WaveFormat
//
//        created by Charley at 2014/12/8 15:04:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.IO;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// GSM 610
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class Gsm610WaveFormat : WaveFormat
    {
        private short samplesPerBlock;

        /// <summary>
        /// Creates a GSM 610 WaveFormat
        /// For now hardcoded to 13kbps
        /// </summary>
        public Gsm610WaveFormat()
        {
            waveFormatTag = WaveFormatEncoding.Gsm610;
            channels = 1;
            averageBytesPerSecond = 1625;
            bitsPerSample = 0; // must be zero
            blockAlign = 65;
            sampleRate = 8000;

            extraSize = 2;
            samplesPerBlock = 320;
        }

        /// <summary>
        /// Samples per block
        /// </summary>
        public short SamplesPerBlock { get { return samplesPerBlock; } }

        /// <summary>
        /// Writes this structure to a BinaryWriter
        /// </summary>
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
        }
    }
}
