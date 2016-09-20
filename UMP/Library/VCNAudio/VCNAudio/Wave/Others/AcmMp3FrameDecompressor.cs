//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3f9dc644-8ae2-4395-829f-85c60f1d570b
//        CLR Version:              4.0.30319.18444
//        Name:                     AcmMp3FrameDecompressor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                AcmMp3FrameDecompressor
//
//        created by Charley at 2014/12/8 15:42:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.NAudio.Wave.Compression;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// MP3 Frame Decompressor using ACM
    /// </summary>
    public class AcmMp3FrameDecompressor : IMp3FrameDecompressor
    {
        private readonly AcmStream conversionStream;
        private readonly WaveFormat pcmFormat;
        private bool disposed;

        /// <summary>
        /// Creates a new ACM frame decompressor
        /// </summary>
        /// <param name="sourceFormat">The MP3 source format</param>
        public AcmMp3FrameDecompressor(WaveFormat sourceFormat)
        {
            this.pcmFormat = AcmStream.SuggestPcmFormat(sourceFormat);
            conversionStream = new AcmStream(sourceFormat, pcmFormat);
        }

        /// <summary>
        /// Output format (PCM)
        /// </summary>
        public WaveFormat OutputFormat { get { return pcmFormat; } }

        /// <summary>
        /// Decompresses a frame
        /// </summary>
        /// <param name="frame">The MP3 frame</param>
        /// <param name="dest">destination buffer</param>
        /// <param name="destOffset">Offset within destination buffer</param>
        /// <returns>Bytes written into destination buffer</returns>
        public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame", "You must provide a non-null Mp3Frame to decompress");
            }
            Array.Copy(frame.RawData, conversionStream.SourceBuffer, frame.FrameLength);
            int sourceBytesConverted;
            int converted = conversionStream.Convert(frame.FrameLength, out sourceBytesConverted);
            if (sourceBytesConverted != frame.FrameLength)
            {
                throw new InvalidOperationException(String.Format("Couldn't convert the whole MP3 frame (converted {0}/{1})",
                    sourceBytesConverted, frame.FrameLength));
            }
            Array.Copy(conversionStream.DestBuffer, 0, dest, destOffset, converted);
            return converted;
        }

        /// <summary>
        /// Disposes of this MP3 frame decompressor
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if (this.conversionStream != null)
                {
                    this.conversionStream.Dispose();
                }
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizer ensuring that resources get released properly
        /// </summary>
        ~AcmMp3FrameDecompressor()
        {
            System.Diagnostics.Debug.Assert(false, "AcmMp3FrameDecompressor Dispose was not called");
            Dispose();
        }
    }
}
