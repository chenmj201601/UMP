//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b7ba6295-44b2-4f39-9ed6-9ed84fb95d5c
//        CLR Version:              4.0.30319.18444
//        Name:                     IMp3FrameDecompressor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                IMp3FrameDecompressor
//
//        created by Charley at 2014/12/8 15:42:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Interface for MP3 frame by frame decoder
    /// </summary>
    public interface IMp3FrameDecompressor : IDisposable
    {
        /// <summary>
        /// Decompress a single MP3 frame
        /// </summary>
        /// <param name="frame">Frame to decompress</param>
        /// <param name="dest">Output buffer</param>
        /// <param name="destOffset">Offset within output buffer</param>
        /// <returns>Bytes written to output buffer</returns>
        int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset);

        /// <summary>
        /// PCM format that we are converting into
        /// </summary>
        WaveFormat OutputFormat { get; }
    }
}
